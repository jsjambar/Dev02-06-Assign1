using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntryPoint
{
#if WINDOWS || LINUX
  public static class Program
  {

    [STAThread]
     static void Main()
	{
			var fullscreen = false;
			Console.WriteLine("Which assignment shall run next? (1, 2, 3, 4, or q for quit)");
			var level = Console.ReadLine();
			if (level == "1")
			{
				var game = VirtualCity.RunAssignment1(SortSpecialBuildingsByDistance, fullscreen);
				game.Run();
			}
			else if (level == "2")
			{
				var game = VirtualCity.RunAssignment2(FindSpecialBuildingsWithinDistanceFromHouse, fullscreen);
				game.Run();
			}
			else if (level == "3")
			{
				var game = VirtualCity.RunAssignment3(FindRoute, fullscreen);
				game.Run();
			}
			else if (level == "4")
			{
				var game = VirtualCity.RunAssignment4(FindRoutesToAll, fullscreen);
				game.Run();
			}
	}

    private static IEnumerable<Vector2> SortSpecialBuildingsByDistance(Vector2 house, IEnumerable<Vector2> specialBuildings)
    {
			// Define the amount of values in the integer array while making the variable
			double[] buildingDistances = new double[specialBuildings.Count()];
			// We start a counter at 0
			int count = 0;

			// We add the distances and the total count in this for loop
			foreach (var v2House in specialBuildings)
			{
				// Distance formula = root of ( (specialBuildingX - houseX)^2) + ((specialBuildingY - houseY)^2) )
				// so example: 
				//root of ( (0 - 5)^2) + ( (0 - 2)^2 )
				//root of (-5 * -5 = 25) + (-2 & -2 = 4)
				// root of (25+4 = 29)
				// distance = 5,385164807......
				// we sort by distance, so....
				var xDist = v2House.X - house.X;
				xDist = xDist * xDist;

				var yDist = v2House.Y - house.Y;
				yDist = yDist * yDist;

				var sumDist = xDist + yDist;
				double distance = Math.Sqrt(sumDist);

				// Add the distance to an array
				buildingDistances[count] = distance;

				// We calculated a distance, so that's another house to the counter
				count++;
			}

			SortMerge(buildingDistances, 0, count - 1);
			return specialBuildings.OrderBy(v => Vector2.Distance(v, house));

    }

	public static void SortMerge(double[] distances, int left, int right)
	{
		if (right > left)
		{
				// to know where to split it up
			int mid = (left + right) / 2;
				// split left side and then the right (indicated by +1)
			SortMerge(distances, left, mid);
			SortMerge(distances, (mid + 1), right);
				MainMerge(distances, left, mid, right);
		}
	}

		static public void MainMerge(double[] numbers, int left, int mid, int right)
		{
			double[] temp = new double[numbers.Count()];

			int left_end = (mid - 1);
			int tmp_pos = left;
			int num_elements = (right - left + 1);

			while ((left <= left_end) && (mid <= right))
			{
				if (numbers[left] <= numbers[mid])
					temp[tmp_pos++] = numbers[left++];
				else
					temp[tmp_pos++] = numbers[mid++];
			}

			while (left <= left_end)
			{
				temp[tmp_pos++] = numbers[left++];
			}

			while (mid <= right)
			{
				temp[tmp_pos++] = numbers[mid++];
			}

			for (int i = 0; i < num_elements; i++)
			{
				numbers[right] = temp[right];
				right--;
			}
		}


		public static float calcDistance(Vector2 uno, Vector2 dos)
		{
			var xDist = dos.X - uno.X;
			xDist = xDist * xDist;

			var yDist = dos.Y - uno.Y;
			yDist = yDist * yDist;

			var sumDist = xDist + yDist;
			float distance = (float)Math.Sqrt(sumDist);

			return distance;
		}



    private static List<List<Vector2>> FindSpecialBuildingsWithinDistanceFromHouse(
      IEnumerable<Vector2> specialBuildings, 
      IEnumerable<Tuple<Vector2, float>> housesAndDistances)
    {
			// Re-wrote it so I can understand what the fuck is even happening.
			List<List<Vector2>> closeB = new List<List<Vector2>>();

			Console.WriteLine(specialBuildings);
			Console.WriteLine(housesAndDistances);

			foreach (var distance in housesAndDistances)
			{
				List<Vector2> currentB = new List<Vector2>();
				foreach (var special in specialBuildings)
				{
					var calcedDistance = calcDistance(distance.Item1, special);
					//Console.WriteLine("Berekend: "+calcedDistance+"... Eigen huis: "+distance.Item2);

					if (calcedDistance <= distance.Item2)
					{
						currentB.Add(special);
					}

					closeB.Add(currentB);
				}
			}
			// Gotta fix this return..
			return closeB;

      /*return
          from h in housesAndDistances
          select
            from s in specialBuildings
						where Vector2.Distance(h.Item1, s) <= h.Item2
            select s;*/
    }

    private static IEnumerable<Tuple<Vector2, Vector2>> FindRoute(Vector2 startingBuilding, 
      Vector2 destinationBuilding, IEnumerable<Tuple<Vector2, Vector2>> roads)
    {
      var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
      List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
      var prevRoad = startingRoad;
      for (int i = 0; i < 30; i++)
      {
        prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, destinationBuilding)).First());
        fakeBestPath.Add(prevRoad);
      }
      return fakeBestPath;
    }

    private static IEnumerable<IEnumerable<Tuple<Vector2, Vector2>>> FindRoutesToAll(Vector2 startingBuilding, 
      IEnumerable<Vector2> destinationBuildings, IEnumerable<Tuple<Vector2, Vector2>> roads)
    {
      List<List<Tuple<Vector2, Vector2>>> result = new List<List<Tuple<Vector2, Vector2>>>();
      foreach (var d in destinationBuildings)
      {
        var startingRoad = roads.Where(x => x.Item1.Equals(startingBuilding)).First();
        List<Tuple<Vector2, Vector2>> fakeBestPath = new List<Tuple<Vector2, Vector2>>() { startingRoad };
        var prevRoad = startingRoad;
        for (int i = 0; i < 30; i++)
        {
          prevRoad = (roads.Where(x => x.Item1.Equals(prevRoad.Item2)).OrderBy(x => Vector2.Distance(x.Item2, d)).First());
          fakeBestPath.Add(prevRoad);
        }
        result.Add(fakeBestPath);
      }
      return result;
    }
  }
#endif
}
