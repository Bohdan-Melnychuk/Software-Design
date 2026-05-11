using System;
using System.Net.Http;

namespace Lab4_PR3
{
    public class NetworkLoadStrategy : IImageLoadStrategy
    {
        public string Load(string href)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(href).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return $"[Завантажено з мережі: {href}]";
                    }
                    else
                    {
                        return $"[Помилка HTTP ({response.StatusCode}): {href}]";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"[Помилка мережі: {ex.Message}]";
            }
        }
    }
}