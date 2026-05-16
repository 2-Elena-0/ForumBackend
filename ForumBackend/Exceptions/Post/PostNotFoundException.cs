namespace ForumBackend.Filters.Post;

public class PostNotFoundException(string massage) : Exception(massage)
{
    
}