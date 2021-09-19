

namespace MotorbikeSpecs.GraphQL.Motorbikes
{
    public record EditMotorbikeInput
    (
        string MotorbikeId,
        string Make,
        string Model,
        string Year,
        string? YouTubeReviewLink);
}
