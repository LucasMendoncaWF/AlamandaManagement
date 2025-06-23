using MongoDB.Driver;
using System.Security.Cryptography;

namespace AlamandaApi.Services.User
{
  public class UserService
  {
    private readonly IMongoCollection<UserModel> _users;

    public UserService(MongoDbSettings settings)
    {
      var client = new MongoClient(settings.ConnectionString);
      var database = client.GetDatabase(settings.DatabaseName);
      _users = database.GetCollection<UserModel>("Users");
    }

    public async Task<UserModel?> GetByEmail(string email)
    {
      return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task Create(UserModel user)
    {
      await _users.InsertOneAsync(user);
    }
    
    public async Task UpdateRefreshToken(UserModel user, string usedRefreshToken, RefreshTokenModel newRefreshToken)
    {
        user.RefreshTokens.RemoveAll(t => t.Token == usedRefreshToken);
        user.RefreshTokens.Add(newRefreshToken);
        var filter = Builders<UserModel>.Filter.Eq(u => u.Id, user.Id);
        var update = Builders<UserModel>.Update.Set(u => u.RefreshTokens, user.RefreshTokens);

        await _users.UpdateOneAsync(filter, update);
    }

    public async Task RemoveRefreshToken(UserModel user, string usedRefreshToken)
    {
        user.RefreshTokens.RemoveAll(t => t.Token == usedRefreshToken);
        var filter = Builders<UserModel>.Filter.Eq(u => u.Id, user.Id);
        var update = Builders<UserModel>.Update.Set(u => u.RefreshTokens, user.RefreshTokens);

        await _users.UpdateOneAsync(filter, update);
    }

    public async Task<UserModel?> GetByRefreshToken(string refreshToken)
    {
      return await _users.Find(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken)).FirstOrDefaultAsync();
    }
  }
}
