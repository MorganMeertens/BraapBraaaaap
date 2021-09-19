
namespace MotorbikeSpecs.GraphQL.Motorbikes
{
    public record AddMotorbikeInput
    (
        string Make,
        string Model,
        string Year,
        string YouTubeReviewLink,
        string CompanyId);
    
}
