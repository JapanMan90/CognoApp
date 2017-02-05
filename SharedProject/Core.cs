using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
namespace SharedProject

{
	public class Core
	{
		private static async Task<Emotion[]> GetHappiness(Stream stream)
		{
			string emotionKey = "0621b84ae7ab4102b39fa0fa199a7edf";

			EmotionServiceClient emotionClient = new EmotionServiceClient(emotionKey);

			var emotionResults = await emotionClient.RecognizeAsync(stream);

			if (emotionResults == null || emotionResults.Count() == 0)
			{
				throw new Exception("Can't detect face");
			}

			return emotionResults;
		}

		//Average happiness calculation in case of multiple people
		public static async Task<float> GetAverageHappinessScore(Stream stream)
		{
			Emotion[] emotionResults = await GetHappiness(stream);

			float score = 0;
			foreach (var emotionResult in emotionResults)
			{
				score = score + emotionResult.Scores.Happiness;
			}

			return score / emotionResults.Count();
		}

		// emoji parser
		public static string ParseUnicodeHex(string hex)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < hex.Length; i += 4)
			{
				string temp = hex.Substring(i, 4);
				char character = (char)Convert.ToInt16(temp, 16);
				sb.Append(character);
			}
			return sb.ToString();
		}

		// The magic for the happ
		public static string GetHappinessMessage(float score)
		{
			float happy = 70;
			float sad = 20;

			score = score * 100;


			float result = (float)Math.Round(score, 1, MidpointRounding.AwayFromZero);
			 

			if (score >= happy)
			{
				// happy happiness
				string emotion = ParseUnicodeHex("d83dde03");
				return result + emotion;
			}
			if (score > sad && score < happy)
			{
				//stright face
				string emotion = ParseUnicodeHex("d83dde10"); 
				return result + emotion;
			}
			else
			{
				// sad
				string emotion = ParseUnicodeHex("d83dde22");
				return result + emotion;
			}
		}

	}
}
