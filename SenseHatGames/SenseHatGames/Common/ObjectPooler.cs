using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace SenseHatGames.Common
{


    /// <summary>
    /// Helpful class to cache objects
    /// </summary>
    public static class ObjectPooler
    {
        /// <summary>
        /// ellipseList caches all the Ellipses
        /// </summary>
        private static List<Ellipse> ellipseList;
        /// <summary>
        /// isUsed list holds a boolean value regarding whether an ellipse in the ellipseList is used (not available) or not
        /// </summary>
        private static List<bool> isUsed;

        /// <summary>
        /// static constructor, will create 4 ellipses
        /// </summary>
        static ObjectPooler()
        {
            ellipseList = new List<Ellipse>();
            isUsed = new List<bool>();
            CreateEllipses(4);

        }

        /// <summary>
        /// Method to create Ellipse XAML objects
        /// </summary>
        /// <param name="count">The number of ellipses to create</param>
        private static void CreateEllipses(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Ellipse el = new Ellipse();
                el.Width = el.Height = 40;
                ellipseList.Add(el);
                isUsed.Add(false);
            }
        }

        /// <summary>
        /// Will mark the ellipse as not used
        /// </summary>
        /// <param name="el">Ellipse to be marked</param>
        public static void RemoveEllipse(Ellipse el)
        {
            isUsed[GetEllipseIsUsedIndex(el)] = false;
        }

        /// <summary>
        /// Fetches a non used Ellipse
        /// If all of the available are used, then the list will be expanded and the first new one will be returned
        /// </summary>
        /// <returns></returns>
        public static Ellipse GetEllipse()
        {
            //find the first non-null ellipse
            var nonNullEllipses = ellipseList.Where(x => isUsed[ellipseList.FindIndex(y => y == x)] == false);
            if(nonNullEllipses.Count() == 0)
            {
                //keep a reference to the old count
                int oldCount = ellipseList.Count();
                //expand the list
                CreateEllipses(2);
                var el = ellipseList.ToArray()[oldCount];  //get the first one that was created
                isUsed[GetEllipseIsUsedIndex(el)] = true; //mark it as used and return it
                return el;
            }
            else
            {
                //find the first one and return it
                var el = nonNullEllipses.First();
                isUsed[GetEllipseIsUsedIndex(el)] = true;
                return el;
            }

        }

        /// <summary>
        /// Helpful method to get an ellipse's corresponding index in the ellipseList list
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        private static int GetEllipseIsUsedIndex(Ellipse el)
        {
            return ellipseList.FindIndex(x => x == el);
        }

       
    }
}
