namespace Jacobi.Services.Storage;

public interface IAnswerStorage
{
    public Task Add(Answer answer);
    public Task<List<Answer>> GetAll();
}