using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace webAPIAutores.Utilidades
{
    public class CabezeraEstaAttribute : Attribute, IActionConstraint
    {
        private readonly string cabecera;
        private readonly string valor;

        public CabezeraEstaAttribute(string cabecera, string valor)
        {
            this.cabecera = cabecera;
            this.valor = valor;
        }
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var cabeceras = context.RouteContext.HttpContext.Request.Headers;

            if (!cabeceras.ContainsKey(cabecera))
            {
                return false;
            }
            return string.Equals(cabeceras[cabecera], valor, StringComparison.OrdinalIgnoreCase);
        }
    }
}
