﻿using Helpers.JWT;
using RedditService_WebRole.Models;
using ServiceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using TableRepository;
using System.Drawing;
using System.IO;
using Helpers;
using System.Security.Cryptography;
using System.Text;

namespace RedditService_WebRole.Controllers
{
    [RoutePrefix("user")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly TableRepositoryKorisnik _userRepository;
        private readonly JwtTokenReader _jwtTokenReader;

        public UserController()
        {
            _userRepository = new TableRepositoryKorisnik();
            _jwtTokenReader = new JwtTokenReader();
        }

        [HttpPut]
        [Route("profile/update")]
        public async Task<IHttpActionResult> UpdateUserProfile([FromBody]Korisnik updatedUser)
        {
            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                if (string.IsNullOrEmpty(emailClaim) || emailClaim != updatedUser.Email)
                    return BadRequest("Email mismatch or invalid token.");

                Korisnik user = await Task.FromResult(_userRepository.DobaviKorisnika(emailClaim));
                if (user == null)
                    return NotFound();

                if (updatedUser != null)
                {
                    user.Ime = updatedUser.Ime ?? user.Ime;
                    user.Prezime = updatedUser.Prezime ?? user.Prezime;
                    user.Adresa = updatedUser.Adresa ?? user.Adresa;
                    user.Grad = updatedUser.Grad ?? user.Grad;
                    user.Drzava = updatedUser.Drzava?? user.Drzava;
                    user.BrTel = updatedUser.BrTel ?? user.BrTel;

                    if (!string.IsNullOrEmpty(updatedUser.Lozinka))
                    {
                        user.Lozinka = HashPassword(updatedUser.Lozinka);
                    }

                    if (!updatedUser.Slika.StartsWith("http"))
                    {
                        Image image;
                        string slikaB64 = updatedUser.Slika;
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(slikaB64.Split(',')[1])))
                        {
                            image = Image.FromStream(ms);
                        }
                        user.Slika = new BlobHelper().UploadImage(image, "slike", Guid.NewGuid().ToString() + ".jpg");
                    }

                    bool isUpdated = await Task.FromResult(_userRepository.IzmeniKorisnika(user.RowKey, user));
                    if (!isUpdated)
                        return BadRequest("Failed to update user profile.");
                }

                return Ok("User profile updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("get")]
        public async Task<IHttpActionResult> GetUser()
        {
            try
            {
                var token = _jwtTokenReader.ExtractTokenFromAuthorizationHeader(Request.Headers.Authorization);
                if (token == null)
                    return Unauthorized();

                var claims = _jwtTokenReader.GetClaimsFromToken(token);
                var emailClaim = _jwtTokenReader.GetClaimValue(claims, "email");

                if (string.IsNullOrEmpty(emailClaim))
                    return BadRequest("No Email Claim");

                Korisnik user = await Task.FromResult(_userRepository.DobaviKorisnika(emailClaim));

                if (user == null)
                    return NotFound();

                user.Lozinka = "";

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #region  Hash Helpers
        static string HashPassword(string password)
        {
            byte[] salt = Encoding.UTF8.GetBytes("CGYzqeN4plZekNC88Umm1Q==");
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Concatenate the salt and hashed password for storage
                byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
                Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
                Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                return Convert.ToBase64String(hashedPasswordWithSalt);
            }
        }
        #endregion
    }
}
