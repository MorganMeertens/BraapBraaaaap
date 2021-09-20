
namespace MotorbikeSpecs.GraphQL.Reviews
{
    public record AddReviewInput(
         string Content,
         string MotorbikeId,
         string BraapUserId);
}
