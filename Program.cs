﻿using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;

namespace ComputerVisionQuickstart
{
    class Program
    {
        // Add your Computer Vision key and endpoint
        // static string key = Environment.GetEnvironmentVariable("VISION_KEY");
        // static string endpoint = Environment.GetEnvironmentVariable("VISION_ENDPOINT");

        
        static string endpoint = "<YourAzureAIServiceEndPoint>";
        static string key = "<YourAzureAIServiceKey>";
    
        //private const string READ_TEXT_URL_IMAGE = "https://cursivehandwriting.co.in/wp-content/uploads/2023/11/f9b3033e7a38c727af188a4604e0c0c4-e1700722488449.jpg";
        private const string READ_TEXT_URL_IMAGE = "https://learn.microsoft.com/azure/ai-services/computer-vision/media/quickstarts/presentation.png";
        //private const string READ_TEXT_URL_IMAGE = "https://raw.githubusercontent.com/Azure-Samples/cognitive-services-sample-data-files/master/ComputerVision/Images/printed_text.jpg";
        

        static void Main(string[] args)
        {
            Console.WriteLine("Azure Cognitive Services Computer Vision - .NET quickstart example");
            Console.WriteLine();

            ComputerVisionClient client = Authenticate(endpoint, key);

            // Extract text (OCR) from a URL image using the Read API
            ReadFileUrl(client, READ_TEXT_URL_IMAGE).Wait();
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public static async Task ReadFileUrl(ComputerVisionClient client, string urlFile)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("READ FILE FROM URL");
            Console.WriteLine();

            // Read text from URL
            var textHeaders = await client.ReadAsync(urlFile);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            Console.WriteLine($"Extracting text from URL file {Path.GetFileName(urlFile)}...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
        }

    }
}