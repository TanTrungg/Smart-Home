using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using ISHE_Utility.Enum;
using ISHE_Utility.Exceptions;
using ISHE_Utility.Helpers.PasswordHasher;
using ISHE_Utility.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ISHE_Service.Implementations
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly AppSetting _appSettings;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSetting> appSettings) : base(unitOfWork, mapper)
        {
            _accountRepository = unitOfWork.Account;
            _roleRepository = unitOfWork.Role;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthViewModel> Authenticated(AuthRequest auth)
        {
            var account = await _accountRepository.GetMany(account => account.PhoneNumber.Equals(auth.PhoneNumber))
                                                .Include(account => account.Role)
                                                .FirstOrDefaultAsync();

            if (account != null && PasswordHasher.VerifyPassword(auth.Password, account.PasswordHash))
            {
                if (!account.Status.Equals(AccountStatus.Active.ToString()))
                {
                    throw new BadRequestException("Tài khoản của bạn đã bị khóa hoặc chưa kích hoạt vui lòng liên hệ admin để mở khóa.");
                }

                var accessToken = GenerateJwtToken(new AuthModel
                {
                    Id = account.Id,
                    Role = account.Role.RoleName,
                    Status = account.Status
                });

                return new AuthViewModel
                {
                    AccessToken = accessToken,
                };
            }
            throw new NotFoundException("Sai tài khoản hoặc mật khẩu.");
        }

        public async Task<AuthViewModel> RefreshAuthentication(string currentToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
                tokenHandler.ValidateToken(currentToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                if (jwtToken.ValidTo > DateTime.Now)
                {
                    //trả về token cũ nếu chưa expire
                    return new AuthViewModel { AccessToken = currentToken };
                }
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var user = await GetAuth(userId);
                
                return new AuthViewModel
                {
                    AccessToken = GenerateJwtToken(user!)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<AuthModel> GetAuth(Guid id)
        {
            var auth = await _accountRepository.GetMany(account => account.Id.Equals(id))
                                                .Include(account => account.Role)
                                                .FirstOrDefaultAsync();
            if (auth != null)
            {
                return new AuthModel
                {
                    Id = auth.Id,
                    Role = auth.Role.RoleName,
                    Status = auth.Status
                };
            }
            throw new NotFoundException("Không tìm thấy account.");
        }

        public async Task<ListViewModel<AccountViewModel>> GetAccounts(AccountFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _accountRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(account => (account.OwnerAccount != null && account.OwnerAccount.FullName.Contains(filter.FullName)) ||
                                                (account.StaffAccount != null && account.StaffAccount.FullName.Contains(filter.FullName)) ||
                                                (account.TellerAccount != null && account.TellerAccount.FullName.Contains(filter.FullName)) ||
                                                (account.CustomerAccount != null && account.CustomerAccount.FullName.Contains(filter.FullName)));
            }

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(account => account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(filter.Role))
            {
                query = query.Where(account => account.Role.RoleName.Equals(filter.Role));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(account => account.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(account => account.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var accounts = await paginatedQuery
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<AccountViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = accounts
            };
        }

        public async Task<AccountViewModel> GetAccount(Guid id)
        {
            return await _accountRepository.GetMany(account => account.Id.Equals(id))
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException($"Không tìm thấy account with id: {id}");
        }
        public async Task<Guid> CreateAccount(string phoneNumber, string password, string role)
        {
            //Check phone number
            var existingUser = await _accountRepository.GetMany(account => account.PhoneNumber.Equals(phoneNumber))
                                                        .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new ConflictException("Số điện thoại đã được sử dụng");
            }

            var accountRole = await _roleRepository.GetMany(ro => ro.RoleName.Equals(role))
                                                            .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy role " + role);

            var passwordHash = PasswordHasher.HashPassword(password);

            var id = Guid.NewGuid();

            var account = new Account
            {
                Id = id,
                PhoneNumber = phoneNumber,
                PasswordHash = passwordHash,
                RoleId = accountRole.Id,
            };
            if (accountRole.RoleName.Equals(AccountRole.Customer))
            {
                account.Status = AccountStatus.Active.ToString();
            }
            else
            {
                account.Status = AccountStatus.Active.ToString();
            }

            _accountRepository.Add(account);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? id : Guid.Empty;
        }

        //PRIVATE METHOD
        private string GenerateJwtToken(AuthModel auth)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", auth.Id.ToString()),

                    new Claim("role", auth.Role.ToString()),

                    new Claim("status", auth.Status.ToString()),
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
