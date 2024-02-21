using AutoMapper;
using Common;
using Common.Dto;
using Common.Models.RequestModels.Patient;
using Common.Models.ResponseModels.Firebase;
using DataAccess.Contexts;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : Controller
	{
		private readonly HospitalAppointmentContext _context;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly AuthService _authService;
		private readonly PatientService _patientService;
		private readonly IMapper _mapper;
		private readonly ILogger<AuthController> _logger;
		private readonly IConfiguration _configuration;

		public AuthController(HospitalAppointmentContext context, IHttpClientFactory httpClientFactory, AuthService authService,
			PatientService patientService, IMapper mapper, ILogger<AuthController> logger, IConfiguration configuration)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
			_authService = authService;
			_patientService = patientService;
			_mapper = mapper;
			_logger = logger;
			_configuration = configuration;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(AccountDto accountDto)
		{
			var firebaseConfig = _configuration.GetSection("FirebaseConfig").Get<FirebaseConfig>();
			var firebaseAuthUrl = firebaseConfig.FirebaseAuthUrl;

			// Firebase Authentication için gerekli istek verisi
			var requestData = new
			{
				email = accountDto.Email,
				password = accountDto.Password,
				returnSecureToken = true
			};

			// HTTP isteği oluştur
			using (var client = _httpClientFactory.CreateClient())
			{
				var response = await client.PostAsJsonAsync(firebaseAuthUrl, requestData);

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var firebaseResponse = JsonConvert.DeserializeObject<FirebaseLoginResponse>(content);
					FirebaseToken decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseResponse.idToken);
					var userIdFromToken = decoded.Uid;
					var user = _authService.GetUserByFirebaseUid(userIdFromToken);

					if (user != null)
					{
						var claims = new Dictionary<string, object>();
						string roleClaim;

						switch (user.Role.Name)
						{
							case "Admin":
								roleClaim = "Admin";
								// Admin yetkisi
								break;

							case "Doctor":
								roleClaim = "Doctor";
								// Doktor yetkisi
								break;

							case "Patient":
								roleClaim = "Patient";
								// Hasta yetkisi
								break;

							default:
								roleClaim = string.Empty;
								break;
						}

						if (!string.IsNullOrEmpty(roleClaim))
						{
							claims.Add(ClaimTypes.Role, roleClaim);
							await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userIdFromToken, claims);
						}

						return Ok(new
						{
							UserId = userIdFromToken,
							Role = user.Role.Name,
							Token = firebaseResponse.idToken,
							Claims = claims
						});
					}
				}

				var errorContent = await response.Content.ReadAsStringAsync();
				throw new Exception();
			}
		}

		[HttpPost("register-patient")]
		[Authorize(Roles = "Admin,Patient")]
		public async Task<IActionResult> RegisterPatient(PatientCreateRequestModel patientCreate)
		{
			var patients = _mapper.Map<PatientDto>(patientCreate);
			var firebaseConfig = _configuration.GetSection("FirebaseConfig").Get<FirebaseConfig>();
			var firebaseAuthUrl = firebaseConfig.FirebaseAuthUrl;

			var requestData = new
			{
				email = patients.Email,
				password = patients.Password,
				returnSecureToken = true
			};

			using (var client = _httpClientFactory.CreateClient())
			{
				try
				{
					var response = await client.PostAsJsonAsync(firebaseAuthUrl, requestData);

					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						var firebaseResponse = JsonConvert.DeserializeObject<FirebaseRegisterResponse>(content);
						var tokenHandler = new JwtSecurityTokenHandler();
						var token = tokenHandler.ReadJwtToken(firebaseResponse.IdToken);
						var userId = token.Subject;

						_patientService.CreatePatient(patients, userId);

						var responseModel = new FirebaseRegisterResponse
						{
							IdToken = firebaseResponse.IdToken,
							Email = firebaseResponse.Email,
							FirebaseUid = userId
						};

						return Ok(responseModel);
					}
					else
					{
						var errorContent = await response.Content.ReadAsStringAsync();
						throw new Exception($"Firebase Patient Registration Error: {errorContent}");
					}
				}
				catch (Exception ex)
				{
					throw new Exception($"An error occurred during patient registration: {ex.Message}");
				}
			}
		}
	}
}