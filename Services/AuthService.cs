using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Services
{
	public class AuthService
	{
		private readonly HospitalAppointmentContext _context;
	

		public AuthService(HospitalAppointmentContext context)
        {
            _context = context;
			
		}

		public Account GetUserByFirebaseUid(string firebaseUid)
		{
			// Veritabanında FirebaseUid'ye göre kullanıcıyı bulmak için Entity Framework kullanımı
			var user = _context.Accounts.Include(s => s.Role).FirstOrDefault(a => a.FirebaseUid == firebaseUid);

			return user;
		}

		// Firebase'den gelen token'dan kullanıcı kimliğini al
		public string GetUserIdFromFirebaseToken(string idToken)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var jwtToken = tokenHandler.ReadJwtToken(idToken);
			return jwtToken.Subject;
		}

		

	}
}
