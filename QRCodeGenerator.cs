using System;
using System.IO;
using System.Text;

namespace qr_code
{
    class Generator
    {

        
        public string message;
        public Mode mode;
        public int version;
        private int[][] qr_code;

        public Generator(string message, Mode mode, int version)
        {
            this.message = message;
            this.mode = mode;
            this.version = version;
        }

        public Generator(ref FileStream stream, Mode mode, int version)
        {
            this.mode = mode;
            this.version = version;
            StreamReader reader = new StreamReader(stream);

            string line;
            StringBuilder sb = new StringBuilder();
            while((line = reader.ReadLine()) != null)
                sb.Append(line);
            this.message = sb.ToString();
            reader.Close();
        }

        public void generate_qr_code()
        {
            
        }
    }
}