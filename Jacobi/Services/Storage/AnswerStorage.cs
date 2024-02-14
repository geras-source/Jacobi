namespace Jacobi.Services.Storage;

public class AnswerStorage : IAnswerStorage
{
    private List<Answer> _storage = new ();

    public Task Add(Answer answer)  
    {
        _storage.Add(answer);
        
        return Task.CompletedTask;
    }

    public Task<List<Answer>> GetAll()
    {
        return Task.FromResult(_storage);
    }
}