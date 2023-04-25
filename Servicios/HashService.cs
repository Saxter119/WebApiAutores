using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using webAPIAutores.DTOs;

namespace webAPIAutores.Servicios
{
    public class HashService
    {
        public ResultadoHash Hash(string textoPlano)
        {
            byte[] sal = new byte[16];

            using(var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }

            return Hash(textoPlano, sal);
        }

        public ResultadoHash Hash(string textoPlano, byte[] sal)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textoPlano, salt: sal, KeyDerivationPrf.HMACSHA256, 10000, 32);

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHash
            {
                Hash = hash,
                Sal = sal
            };
        }
    }
}