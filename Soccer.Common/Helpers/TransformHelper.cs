using Soccer.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace Soccer.Common.Helpers
{
    public class TransformHelper : ITransformHelper
    {
        public List<Group> ToGroups(List<GroupResponse> response)
        {
            List<Group> list = new List<Group>(); //crea una lista vacìa

            foreach (GroupResponse groupResponse in response) //recorremos la lista de grupos
            {
                Group group = new Group(); //y por cada grupo

                //recorremos el detalle de grupos de acuerdo a los criterios
                foreach (GroupDetailResponse detail in groupResponse.GroupDetails
                                                       .OrderByDescending(gd => gd.Points)
                                                       .ThenByDescending(gd => gd.GoalsDifference)
                                                       .ThenByDescending(gd => gd.GoalsFor)) 
                {
                    group.Add(detail); //adicionamos el detalle de grupo
                }

                group.Name = groupResponse.Name;
                list.Add(group); //añdimos el grupo a la lista
            }

            return list;
        }
    }
}
