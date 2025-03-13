using System.ComponentModel.DataAnnotations;

namespace EasyFitHub.Services
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Serviço usado na validação de formatação de Strings
    /// </summary>
    public class ValidationService
    {
        /// <summary>
        /// Verifica se uma string encontra-se delimitada corretamente
        /// </summary>
        /// <param name="str">O texto</param>
        /// <param name="maxLen">O tamanho máx de carateres</param>
        /// <param name="minLen">O tamanho mín de caratéres</param>
        /// <returns>True ou False consoante se a String encontra-se formatada corretamente</returns>
        public bool ValidateString(string str, int maxLen, int minLen = 0) 
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length > maxLen || str.Length < minLen)
                return false;

            return true;
        }
        /// <summary>
        /// Verifica se uma String possui formato Email
        /// </summary>
        /// <param name="email">o texto da string</param>
        /// <param name="maxLen">O tamanho máximo para o email</param>
        /// <param name="minLen">O tamanho minimo</param>
        /// <returns>True ou False, consoante se o Email encontra-se devidamente formatado</returns>
        public bool ValidateEmail(string email, int maxLen, int minLen = 0)
        {
            if (!ValidateString(email, maxLen, minLen)) return false;
            
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        
    }
}
