using System.Security.Cryptography;
using System.Text;

namespace WebVisitsMobile.Services.Services.Encriptacion
{
    public class EncriptadorAESService
    {
        SymmetricAlgorithm oAlgoritmo;
        List<String> arrClaves = new List<String>();
        List<String> arrIVs = new List<String>();
        byte[] ClaveGenerada;
        byte[] IV;
        int IndiceClave = -1;
        int IndiceIV = -1;

        public EncriptadorAESService()
        {
            this.oAlgoritmo = SymmetricAlgorithm.Create("Rijndael");

            this.arrClaves.Add("5C9C97C3D14504329C6312D5A1C435E7CC1B144E32F9590296C642A3476C6E11");
            this.arrClaves.Add("CF434B2A99732573023ED4285B6C9728CF22B51D966158BE495C84FB9A58C713");
            this.arrClaves.Add("7F539C8B53A3C2E1FFFA42A7AC075F8B3D767AC176433183E3213A87F4EAB423");
            this.arrClaves.Add("0D70F44D5CE9BA3018492BFCC51A764F20C472089C21B9BD3A110E00105FB473");
            this.arrClaves.Add("65AE5A9245ABBC9BFA93348803D8D05D0E33D194F7478FD1FC219FAC845485FF");
            this.arrClaves.Add("97EBEDF071C72FBC4FEAF261A0EB186D06DE7B2A10D3B8786AB84ADE3EC4CF79");
            this.arrClaves.Add("7B0C4C79D8629595849262EEF3303D9C65ABD6F35EBA6F9C731D4A6CC835BC40");
            this.arrClaves.Add("60BEFCA6175BE324FCD6FA1C17BE1FA90AE920C6B99198A94B7AA5DFCEC998E8");
            this.arrClaves.Add("360209B8DC9488B84463F7E734A56F6B7B224CDAD7ED61F6A21B8E6F7C270687");
            this.arrClaves.Add("4982BE1CC21982A169F8E51FD22F3DB289208BE0279251A1352D2489F329AEEA");
            this.arrClaves.Add("ED9DCB027F030F6E333FB195FC93095F4B3EB92D8978E0909A723E74F57C71E2");
            this.arrClaves.Add("1640A8DF50AD41B2E7F9974FEB4B34D060456EF5AACDF69CE66CE8250817E7AB");
            this.arrClaves.Add("F1FE049BC37056FBC652D84D5C61E846E7AC9E61E7C6F18537DB2BF58F5A703F");
            this.arrClaves.Add("B16A7012D08EA4CEBD7696912121225B104B72FE653D429DD9E1F392F37B7FB2");
            this.arrClaves.Add("75C018FD0B1622011E9AF24E677B243995EA0D10E4877B5068B80C8580B7BFBE");
            this.arrClaves.Add("018583E4E10E176F39FA02FB30CF048C29D7F469D2F60DE0551FE7ED64CAC326");
            this.arrClaves.Add("8DC461A9ABEA4294454DDB8C8A8F0482B5E6CBF7AB80C0ABC19A99BA3AB65D89");
            this.arrClaves.Add("9678A6352F6FF8FB243175BF362313357C7635A6F1AC7D54FA9F0E810859368D");
            this.arrClaves.Add("C4AEB9643DEF39A0E7A4FFFC7F1E0468BFEABD8AD5101A39631B9D7A9B07B160");
            this.arrClaves.Add("85BA6ABAE35610D3597858BB694D8681D4B8D1E9420B163CCBAC2A7D63D7FA7F");
            this.arrClaves.Add("22859ACD97BC42EF911807B7BD64FAAC92E5ED62B639AB12D4A00F43CEB8B04C");
            this.arrClaves.Add("0CA87BBC36F25EA0933E18094539CE6FB30631E3F4B54024E24C299B721779C8");
            this.arrClaves.Add("18EAA4F80544BA047620B17EA7E8E4D5D5B3CB7B2B8D4807FE060AB09B738A04");
            this.arrClaves.Add("DAD012F5C590032B56063EA0F40785C3F1A8D78140FCE641ED102159D7151E58");
            this.arrClaves.Add("32AE0546BE7CC44FDB069E6B2DBF9ABAEB1541269B642C358FC8B9960C53C50E");
            this.arrClaves.Add("6A99C5DCB0A9CEA411D21B44AE7A12C8D00463491B7AF5AC8EDA509EC56CEB54");
            this.arrClaves.Add("10342702C82CC8CEB0C4564559654811BFCEC16D5F97011578647006F0DDA5EC");
            this.arrClaves.Add("F3CB52F33F642A7E8FA13B185D23D90204F424C3B3925D4EF064A274487BA88F");
            this.arrClaves.Add("19482CC899D22DB3364857EF5320291A6615A2FB2BD11A9B4BB2E594A6087A04");
            this.arrClaves.Add("D934B2F6B891042140BF605075862D68C9B96B92F7A4A65FDBB325C038E6CE9E");

            this.arrIVs.Add("07FB6A64CC39852084961898C93B0EC5");
            this.arrIVs.Add("B9726AC4879B9014E6441776F674BDEB");
            this.arrIVs.Add("BDC00AB38F38D07FE5AAA6C6C8F70C5B");
            this.arrIVs.Add("8B29008D7DF55A75C0AAC0400DBF1C0D");
            this.arrIVs.Add("920A08ED42ADEAEF55D7F75C8AD356BC");
            this.arrIVs.Add("CE6815D7DBD18F5DE708F0CF49296C0E");
            this.arrIVs.Add("4AEAE36187FA62216854C16BA6D282C7");
            this.arrIVs.Add("A9DA0AC5111963628445154E1082E5FD");
            this.arrIVs.Add("28341C7CBCC4514D6D21CCD5A06D64F8");
            this.arrIVs.Add("2D14895FBA3040B0E70250B17B0F6756");
            this.arrIVs.Add("D141AD08AB1E6F4B4FA4E4C124292120");
            this.arrIVs.Add("21A09F2ABE77F38080B3CE3A8DCD7DA4");
            this.arrIVs.Add("73F85EED039E256DD9AB62BE4486C9AF");
            this.arrIVs.Add("B9C8AA860C4363CA7A480429D6D3F01B");
            this.arrIVs.Add("D222600B3106A87124C959FBF0DE7F48");
            this.arrIVs.Add("51616166186B75990D23398448A2B83C");
            this.arrIVs.Add("7A5F15213F3397F601C01783E691D047");
            this.arrIVs.Add("5365652D23BE05BACA8C75C36DD1779D");
            this.arrIVs.Add("8FA65C744CB340BD567808522D6AD8B8");
            this.arrIVs.Add("79754307196B51A1699DED2B20AAC209");
            this.arrIVs.Add("3E73F24BE18C148ACB82B74A7D65AC48");
            this.arrIVs.Add("2E0D025BD00B90B84579399A58711A25");
            this.arrIVs.Add("822CE2437B658AD50B3E059F7DA5F7B3");
            this.arrIVs.Add("A3AD4898A7F124052C07EAE54C520688");
            this.arrIVs.Add("3F36528DEBEDEC20125037376645C212");
            this.arrIVs.Add("6B3180EC6186EAF692CCC40916D8AB47");
            this.arrIVs.Add("5D7CD1E249F59D73E7DDD947829FED90");
            this.arrIVs.Add("55292DD08A3F3ABD305A8A090E503F87");
            this.arrIVs.Add("29824B8E989EEDDFE827473E4FC9A8DA");
            this.arrIVs.Add("124339FBD9A9705B6A4DEF2C50F929C3");

        }
        public void Configurar()
        {
            this.oAlgoritmo.BlockSize = 128;
            this.oAlgoritmo.Mode = CipherMode.CBC;
            this.oAlgoritmo.Padding = PaddingMode.PKCS7;
            this.oAlgoritmo.KeySize = 256;
        }

        public void GenerarClave()
        {
            this.oAlgoritmo.Key = this.GenerarClave_bytes();
            this.ClaveGenerada = this.oAlgoritmo.Key;
        }

        public void GenerarIV()
        {
            this.oAlgoritmo.IV = this.GenerarIV_bytes();
            this.IV = this.oAlgoritmo.IV;
        }

        public byte[] Encriptar(String strMensaje)
        {
            ICryptoTransform oEncriptador = oAlgoritmo.CreateEncryptor();
            byte[] textoPlano = Encoding.Default.GetBytes(strMensaje);
            MemoryStream oMemoria = new MemoryStream();
            CryptoStream oCryptoStream = new CryptoStream(oMemoria, oEncriptador, CryptoStreamMode.Write);
            oCryptoStream.Write(textoPlano, 0, strMensaje.Length);
            oCryptoStream.FlushFinalBlock();

            oMemoria.Close();
            oCryptoStream.Close();

            return oMemoria.ToArray();
        }

        public String Encriptar_String(String strMensaje)
        {
            byte[] encriptado = this.Encriptar(strMensaje);
            return this.ByteArrayToString(encriptado);
        }

        public String Desencriptar_String(String strTexto)
        {
            return this.Desencriptar(this.StringToByteArray(strTexto));
        }

        public string Desencriptar(byte[] cipherText)
        {

            // Check arguments. 
            //if (cipherText == null || cipherText.Length <= 0)
            //this.oMiLog.Escribir("ENCRIPTADOR: no hay texto a desencriptar");
            //if (this.ClaveGenerada == null || this.ClaveGenerada.Length <= 0)
            //this.oMiLog.Escribir("ENCRIPTADOR: no hay clave");
            //if (this.IV == null || this.IV.Length <= 0)
            //this.oMiLog.Escribir("ENCRIPTADOR: no hay IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            ICryptoTransform decryptor = oAlgoritmo.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream 
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;

        }

        public byte[] GenerarClave_bytes()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 29);
            this.IndiceClave = randomNumber;

            return StringToByteArray(this.arrClaves[randomNumber]);
        }

        public byte[] GenerarIV_bytes()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 29);
            this.IndiceIV = randomNumber;

            return StringToByteArray(this.arrIVs[randomNumber]);
        }

        public byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public String Get_Clave_String()
        {
            return this.ByteArrayToString(this.ClaveGenerada);
        }

        public String Get_IV_String()
        {
            return this.ByteArrayToString(this.IV);
        }

        public String ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public int Get_IndiceIV()
        {
            return this.IndiceIV;
        }

        public int Get_IndiceClave()
        {
            return this.IndiceClave;
        }

        public void LoadIV(int numIV)
        {
            this.IV = this.StringToByteArray(this.arrIVs[numIV]);
            this.oAlgoritmo.IV = this.IV;
        }

        public void LoadClave(int numClave)
        {
            this.ClaveGenerada = this.StringToByteArray(this.arrClaves[numClave]);
            this.oAlgoritmo.Key = this.ClaveGenerada;
        }
    }
}