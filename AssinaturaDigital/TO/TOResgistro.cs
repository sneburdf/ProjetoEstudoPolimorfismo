using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssinaturaDigital.TO
{
    public abstract class TORegistro
    {
        public abstract string Chave();
        public abstract void Ler(string line);

    }
}
