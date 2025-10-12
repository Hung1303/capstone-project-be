using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITokenService
    {
        void GenerateRefreshToken(Token token);
        Token GetRefreshToken(string refreshToken);
        Token GetRefreshTokenByUserID(Guid id);
        void RemoveAllRefreshToken();
        void ResetRefreshToken();
        void UpdateRefreshToken(Token refreshToken);
    }

}
