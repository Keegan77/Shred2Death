using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BowlFinder //todo: script is deprecated, but im keeping it in the project cause the code is actually pretty neat
{
    public static BowlMeshGenerator GetNearestBowlToObject(GameObject obj)
    {
        RaycastHit[] hits = Physics.SphereCastAll(obj.transform.position, 2f, Vector3.down, 0);

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.GetComponent<BowlMeshGenerator>())
            {
                Debug.Log(hit.collider.gameObject.name);
                return hit.collider.gameObject.GetComponent<BowlMeshGenerator>();
            }
        } // else we return null
        return null;
    } // TODO: make this return the bowl only if the bowl is under the player game object
    
    public static Vector3[] GetClosestTwoVertsToPlayer(BowlMeshGenerator bowl, GameObject player)
    {
        Vector3 iterationalClosestVert = Vector3.positiveInfinity;
        foreach (var vert in bowl.topVerts)
        {
            var dist = Vector3.Distance(vert, player.transform.position);
            
            if (dist < Vector3.Distance(iterationalClosestVert, player.transform.position))
            {
                iterationalClosestVert = vert;
            }
        }
        Vector3 closestVert = iterationalClosestVert;
        iterationalClosestVert = Vector3.positiveInfinity;
        
        foreach (var vert in bowl.topVerts)
        {
            var dist = Vector3.Distance(vert, player.transform.position);
            
            if (dist < Vector3.Distance(iterationalClosestVert, player.transform.position) && vert != closestVert)
            {
                iterationalClosestVert = vert;
            }
        }
        Vector3 secondClosestVert = iterationalClosestVert;

        Debug.Log($"Closest Vert: {closestVert}");
        Debug.Log($"2nd Closest Vert: {secondClosestVert}");
        return new Vector3[] { closestVert, secondClosestVert };
    } 
    //TODO: make sure player is close enough to the edge of the bowl to be able to jump off of it

    public static Vector3 ClosestPointBetweenVertsToPlayer(Vector3 playerPos, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        lineDirection.Normalize();
        Vector3 projectedPoint = Vector3.Project(playerPos - lineStart, lineDirection) + lineStart;
        
        //debug
        Debug.Log($"Projected Point: {projectedPoint}");
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = projectedPoint;
        sphere.transform.localScale = new Vector3(1f, 1f, 1f); // Set the size of the sphere

        return projectedPoint;
    }
    
    // iterate through the nearest bowl's vertices and find the two closest to the player
    
    
    
}
