using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            HashSet<Point> chests = new HashSet<Point>(map.Chests.Select(p=>p.Location));
            var pathToExit = BfsTask.FindPaths(map, map.InitialPosition, new[] { new Chest(map.Exit,0) }).FirstOrDefault();
            if (pathToExit != null)
                foreach (var point in pathToExit)
                    if (chests.Contains(point))
                        return ConvertToDirections(pathToExit);
            var pathsToChests = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
            SinglyLinkedList<Point> shortestPath = null;
            foreach (var pathsToChest in pathsToChests)
            {
                var pathFromChestToExit = FindPath(map, pathsToChest, map.Exit);
                if (shortestPath == null || (pathFromChestToExit != null && pathFromChestToExit.Length < shortestPath.Length))
                    shortestPath = pathFromChestToExit;
            }
            if (shortestPath == null)
                shortestPath = pathToExit;
            return ConvertToDirections(shortestPath);
        }
        private static MoveDirection[] ConvertToDirections(SinglyLinkedList<Point> path)
        {
            if (path?.Previous == null)
                return Array.Empty<MoveDirection>();

            var directions = new List<MoveDirection>();
            var current = path;
            var previous = path.Previous;

            while (previous != null)
            {
                var dx = current.Value.X - previous.Value.X;
                var dy = current.Value.Y - previous.Value.Y;

                if (dx == 1) directions.Add(MoveDirection.Right);
                else if (dx == -1) directions.Add(MoveDirection.Left);
                else if (dy == 1) directions.Add(MoveDirection.Down);
                else if (dy == -1) directions.Add(MoveDirection.Up);

                current = previous;
                previous = previous.Previous;
            }
            directions.Reverse();
            return directions.ToArray();
        }
        
        static SinglyLinkedList<Point> FindPath(Map map, SinglyLinkedList<Point> start, Point end)
        {
            var track = new Dictionary<Point, SinglyLinkedList<Point>>();
            track[start.Value] = start;
            var queue = new Queue<SinglyLinkedList<Point>>();
            queue.Enqueue(start);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                var incidentNodes = Walker.PossibleDirections
                    .Select(direction => node.Value + direction)
                    .Where(point => map.InBounds(point) && map.Dungeon[point.X, point.Y] != MapCell.Wall);
                foreach (var nextNode in incidentNodes)
                {
                    if (track.ContainsKey(nextNode)) continue;
                    track[nextNode] = new SinglyLinkedList<Point>(nextNode, node);
                    queue.Enqueue(track[nextNode]);
                }
                if (track.ContainsKey(end)) return track[end];
            }
            if (!track.ContainsKey(end)) return null;
            throw new Exception("Never exception");
        }
    }
}