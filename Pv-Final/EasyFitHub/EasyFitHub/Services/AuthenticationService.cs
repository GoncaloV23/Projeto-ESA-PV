using EasyFitHub.Data;
using EasyFitHub.Models.Account;

namespace EasyFitHub.Services
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// 
    /// Serviço usado na verificação de authenticaçao
    /// </summary>
    public class AuthenticationService
    {

        private AuthenticationsInfo _authenticationInfo;

        public AuthenticationService(EasyFitHubContext context)
        {
            _authenticationInfo = new AuthenticationsInfo(context);
        }
        /// <summary>
        /// devolve uma conta consoante o value
        /// </summary>
        /// <param name="propertyName">nome da propriedade a comparar</param>
        /// <param name="value">o valor da propriedade</param>
        /// <returns>A conta em si</returns>
        public async Task<Account?> GetAccount(string propertyName, string? value)
        {
            if (value == null) return null;

            Account? account;
            switch (propertyName)
            {
                case "id":
                    account = (int.TryParse(value, out int valueAsInt)) ?
                        await _authenticationInfo.GetAccount(valueAsInt) :
                        null;
                    break;


                case "userName":
                    account = await _authenticationInfo.GetAccount(value);
                    break;

                case "token":
                    account = await _authenticationInfo.GetAccountWithToken(value);
                    break;

                default:
                    account = null;
                    break;
            }

            return account;

        }
    }
}
