using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dungeon;

public class BfsTask
{
    public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
    {
        var queue = new Queue<SinglyLinkedList<Point>>();
        var visited = new HashSet<Point>();
        var chestsSet = new HashSet<Point>(chests.Select(c => c.Location));
        var foundChests = new List<SinglyLinkedList<Point>>();

        queue.Enqueue(new SinglyLinkedList<Point>(start, null));
        visited.Add(start);
        Cycle(queue, map, foundChests, visited, chestsSet);
        return foundChests;
    }
    public static void Cycle(Queue<SinglyLinkedList<Point>> queue,Map map, List<SinglyLinkedList<Point>> foundChests, HashSet<Point> visited, HashSet<Point> chestsSet) 
    {
        while (queue.Count > 0 && chestsSet.Count > 0)
        {
            var currentPath = queue.Dequeue();
            var currentPoint = currentPath.Value;

            if (chestsSet.Contains(currentPoint))
            {
                chestsSet.Remove(currentPoint);
                foundChests.Add(currentPath);
            }

            if (chestsSet.Count == 0) break;

            foreach (var neighbor in GetNeighbors(currentPoint, map))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(new SinglyLinkedList<Point>(neighbor, currentPath));
                }
            }
        }
    }

    private static IEnumerable<Point> GetNeighbors(Point point, Map map)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            var newX = point.X + dx[i];
            var newY = point.Y + dy[i];
            var newPoint = new Point(newX, newY);

            if (newX >= 0 && newY >= 0 && newX < map.Dungeon.GetLength(0) && newY < map.Dungeon.GetLength(1) &&
                map.Dungeon[newX, newY] == MapCell.Empty)
            {
                yield return newPoint;
            }
        }
    }
}
