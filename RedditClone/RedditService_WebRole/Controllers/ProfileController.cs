using Contracts;
using RedditService_WebRole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TableRepository;


namespace RedditService_WebRole.Controllers
{
    [RoutePrefix("izmenaProfila")]
    public class ProfileController : ApiController
    {
        TableRepositoryKorisnik repoKor = new TableRepositoryKorisnik();

      
        [HttpPut]
        [Route("korisnici")]
        public async Task<IHttpActionResult> UpdateKorisnik(ProfileChange update)
        {
            if (update == null || string.IsNullOrEmpty(update.OldEmail))
            {
                return BadRequest("Invalid data.");
            }

            var existingKorisnik = repoKor.DobaviKorisnika(update.OldEmail);
            if (existingKorisnik == null)
            {
                return NotFound();
            }

            // Provera da li novi email postoji i da nije već registrovan kod drugog korisnika
            if (!string.IsNullOrEmpty(update.Email) && update.Email != existingKorisnik.Email)
            {
                var korisnikSaNovimEmailom = repoKor.DobaviKorisnika(update.Email);
                if (korisnikSaNovimEmailom != null)
                {
                    existingKorisnik.Email = update.Email ?? existingKorisnik.Email;
                }
            }

            existingKorisnik.Ime = update.FirstName ?? existingKorisnik.Ime;
            existingKorisnik.Prezime = update.LastName ?? existingKorisnik.Prezime;
            existingKorisnik.Adresa = update.Address ?? existingKorisnik.Adresa;
            existingKorisnik.Grad = update.City ?? existingKorisnik.Grad;
            existingKorisnik.Drzava = update.Country ?? existingKorisnik.Drzava;
            existingKorisnik.BrTel = update.Phone ?? existingKorisnik.BrTel;
            //existingKorisnik.Email = update.Email ?? existingKorisnik.Email;
            existingKorisnik.Lozinka = update.Password ?? existingKorisnik.Lozinka;  
            existingKorisnik.Slika = update.Image ?? existingKorisnik.Slika;  



            var result = await Task.FromResult(repoKor.IzmeniKorisnika(update.OldEmail, existingKorisnik));
            if (!result)
            {
                return InternalServerError();
            }

            return Ok(existingKorisnik);
        }
    }
}
