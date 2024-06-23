using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ATBMTT_RSA
{
    public partial class Form1 : Form
    {
        
        private BigInteger P, Q, N, Phi, E, D;
        public Form1()
        {
            InitializeComponent();
            btn_laykhoa.Click += new EventHandler(btn_laykhoa_Click);
            btn_MaHoa.Click += new EventHandler(btn_MaHoa_Click);
            btn_GiaiMa.Click += new EventHandler(btn_GiaiMa_Click);
            btn_laybanro.Click += new EventHandler(btn_laybanro_Click);
            Ghibanmahoa.Click += new EventHandler(Ghibanmahoa_Click);
            btn_laybnamahoa2.Click += new EventHandler(btn_laybnamahoa2_Click);
            btn_ghibanro2.Click += new EventHandler(btn_ghibanro2_Click);
        }
        private void btn_laykhoa_Click(object sender, EventArgs e)
        {
            
            if (cb_SK.Checked == true) { 
            // Lấy khóa công khai dưới dạng chuỗi XML
                btn_laykhoatudong_Click(sender, e);
        }
            else
            {
                btn_laykhoathucong_Click(sender, e);
            }
        }


        // Phương thức lấy tự động khóa 
        private void btn_laykhoatudong_Click(object sender, EventArgs e)
        {

            P = GenerateRandomPrime(1024);
            do
            {
                Q = GenerateRandomPrime(1024);
            }
            while (P == Q);

            txt_P.Text = P.ToString();
            txt_Q.Text = Q.ToString();
            if (IsProbablePrime(P, 40) && IsProbablePrime(Q, 40))
            {
                N = P * Q;
                Phi = (P - 1) * (Q - 1);

                do
                {
                    E = RandomBigInteger(2, Phi);
                } while (GCD(E, Phi) != 1);

                D = ModInverse(E, Phi);
                txt_KhoaCongKhai.Text = E.ToString();
                txt_KhoaBiMat.Text = D.ToString();
            }
            else
            {
                MessageBox.Show("P và Q phải là số nguyên tố.");
            }

        }

        // kiểm tra số nguyên tố theo định ly fermart
        static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
        {
            if (min >= max)
                throw new ArgumentException("min phải nhỏ hơn max");

            byte[] bytes = max.ToByteArray();
            BigInteger result;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                do
                {
                    rng.GetBytes(bytes);
                    bytes[bytes.Length - 1] &= (byte)0x7F;
                    result = new BigInteger(bytes);
                } while (result < min || result >= max);
            }
            return result;
        }

        public static bool IsProbablePrime(BigInteger n, int k)
        {
            if (n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[n.ToByteArray().Length];
                for (int i = 0; i < k; i++)
                {
                    BigInteger a;
                    do
                    {
                        rng.GetBytes(bytes);
                        a = new BigInteger(bytes);
                    } while (a < 2 || a >= n - 2);

                    a = BigInteger.ModPow(a, n - 1, n);
                    if (a != 1)
                        return false;
                }
            }

            return true;
        }

        public static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max, RandomNumberGenerator rng)
        {
            byte[] bytes = new byte[max.ToByteArray().Length];
            BigInteger number;

            do
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= 0x7F; // Bảo đảm số luôn dương
                number = new BigInteger(bytes);
            } while (number < min || number > max);

            return number;
        }

        public static BigInteger GenerateRandomPrime(int bitLength)
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                while (true)
                {
                    byte[] bytes = new byte[bitLength / 8];
                    rng.GetBytes(bytes);
                    bytes[bytes.Length - 1] &= 0x7F; // Bảo đảm số luôn dương
                    BigInteger candidate = new BigInteger(bytes);

                    candidate |= BigInteger.One << (bitLength - 1); // Đặt bit cao nhất để đảm bảo độ dài bit chính xác
                    candidate |= BigInteger.One; // Đặt bit thấp nhất để đảm bảo lẻ

                    if (IsProbablePrime(candidate, 50)) // Sử dụng kiểm tra Miller-Rabin với độ chắc chắn cao
                    {
                        return candidate;
                    }
                }
            }
        }
        // kết thúc lấy tự động khóa

        // Hàm lấy thủ công khác mỗi tự động là nhập vào P và Q
        private void btn_laykhoathucong_Click(object sender, EventArgs e)
        {
            P = BigInteger.Parse(txt_P.Text);
            Q = BigInteger.Parse(txt_Q.Text);

            if (IsProbablePrime(P, 50) && IsProbablePrime(Q, 50))
            {
                N = P * Q;
                Phi = (P - 1) * (Q - 1);

                do
                {
                    E = RandomBigInteger(2, Phi);
                } while (GCD(E, Phi) != 1);

                D = ModInverse(E, Phi);

                txt_KhoaCongKhai.Text = E.ToString();
                txt_KhoaBiMat.Text = D.ToString();
            }
            else
            {
                MessageBox.Show("P và Q phải là số nguyên tố.");
            }
        }
        
        private void btn_MaHoa_Click(object sender, EventArgs e)
        {
            string banro = txt_banro1.Text;
            BigInteger[] br = StringToBigIntegerArray(banro); // Convert input string to BigInteger array

            string khoaCongKhai = txt_KhoaCongKhai.Text;

            if (string.IsNullOrEmpty(banro) || string.IsNullOrEmpty(khoaCongKhai))
            {
                MessageBox.Show("Bạn chưa nhập dữ liệu hoặc lấy khóa công khai.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                BigInteger[] kq = new BigInteger[br.Length]; // Initialize result array with the same length as input
                String kk ="";
                for (int i = 0; i < br.Length; i++)
                {
                    BigInteger s = BigInteger.ModPow(br[i],E, N); // Encrypt each BigInteger number
                    kq[i] = s;
                    kk += s.ToString() +'_';
                }
                
                // Convert BigInteger array back to string
               txt_banmahoa1.Text =kk; // Display encrypted text
            }
        }
        private void btn_GiaiMa_Click(object sender, EventArgs e)
        {
            string banmahoa = txt_banmahoa2.Text;
            string filteredInput = new string(Array.FindAll(banmahoa.ToCharArray(), c => char.IsDigit(c) || c == '_'));
            string[] input = filteredInput.Split('_');// Convert encrypted string to BigInteger array

            BigInteger[] bm = new BigInteger[input.Length - 1];

            for (int i = 0; i < bm.Length; i++)
            {
                bm[i] = BigInteger.Parse(input[i]);

            }
            string khoaBiMat = txt_KhoaBiMat.Text;

            if (string.IsNullOrEmpty(banmahoa) || string.IsNullOrEmpty(khoaBiMat))
            {
                MessageBox.Show("Bạn chưa nhập dữ liệu hoặc lấy khóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                BigInteger[] kq = new BigInteger[bm.Length]; // Initialize result array with the same length as input

                for (int i = 0; i < bm.Length; i++)
                {
                    BigInteger s = BigInteger.ModPow(bm[i], D, N); // Decrypt each BigInteger number
                    kq[i] = s;
                }

                string kqc = BigIntegerArrayToString(kq); // Convert BigInteger array back to string
                txt_banro2.Text = kqc;

                // Display decrypted text
            }
        }
        // Hàm chuyển mảng các số BigInteger về chuỗi

        public static BigInteger[] StringToBigIntegerArray(string str)
        {
            // Chuyển chuỗi thành mảng byte sử dụng mã UTF-8
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            // Khởi tạo mảng BigInteger có độ dài bằng độ dài của mảng byte
            BigInteger[] bigIntegers = new BigInteger[bytes.Length];

            // Chuyển đổi từng byte thành số BigInteger và lưu vào mảng BigInteger
            for (int i = 0; i < bytes.Length; i++)
            {
                bigIntegers[i] = bytes[i]; // Mỗi byte là một số BigInteger
            }

            return bigIntegers;
        }



        public static string BigIntegerArrayToString(BigInteger[] bigIntegers)
        {
            // Khởi tạo mảng byte từ mảng số BigInteger
            byte[] bytes = new byte[bigIntegers.Length];

            // Chuyển từng số BigInteger thành byte và lưu vào mảng byte
            for (int i = 0; i < bigIntegers.Length; i++)
            {
                bytes[i] = (byte)bigIntegers[i];
            }

            // Chuyển mảng byte thành chuỗi sử dụng mã UTF-8
            string str = Encoding.UTF8.GetString(bytes);

            return str;
        }
        public static BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger t = 0;
            BigInteger newt = 1;
            BigInteger r = n;
            BigInteger newr = a;

            while (newr != 0)
            {
                BigInteger quotient = r / newr;

                BigInteger temp;
                temp = t;
                t = newt;
                newt = temp - quotient * newt;

                temp = r;
                r = newr;
                newr = temp - quotient * newr;
            }

            // Nếu r > 1, không có nghịch đảo
            if (r > 1)
            {
                throw new ArgumentException("Không có nghịch đảo");
            }

            // Nếu t nhỏ hơn 0, chuyển thành dương bằng cách cộng n
            if (t < 0)
            {
                t = +n;
            }

            return t;
        }

     

        private BigInteger GCD(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
       





        private void btn_laybanro_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                txt_banro1.Text = System.IO.File.ReadAllText(filePath);
            }
        }

        private void Ghibanmahoa_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                System.IO.File.WriteAllText(filePath, txt_banmahoa1.Text);
            }
        }

        private void btn_laybnamahoa2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                txt_banmahoa2.Text = System.IO.File.ReadAllText(filePath);
            }
        }

        private void btn_ghibanro2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                System.IO.File.WriteAllText(filePath, txt_banro2.Text);
            }
        }       
    }
}
