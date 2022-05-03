using UnityEditor;

class WebGLBuilder
{
    static void build ()
    {
        string[] scenes = { "Assets\\Scenes\\Loader.unity",
        					"Assets\\Scenes\\Login.unity",
        					"Assets\\Scenes\\Home.unity",
        					"Assets\\Scenes\\Lobby.unity",
        					"Assets\\Scenes\\Game.unity" };
        
        string pathToDeploy = "Build";  
        
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
    }
}