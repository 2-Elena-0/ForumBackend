using ForumBackend.Contracts.Topic;
using ForumBackend.Filters.Topic;
using ForumBackend.Services.Topic;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopicController(ITopicService topicService, ILogger<TopicController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TopicResponseContract[]>> GetAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all topics.");

        var topics = await topicService.GetAllAsync(cancellationToken);

        logger.LogInformation("topics received {Count} topics.", topics.Count);

        return Ok(topics);
    }

    [TopicExceptionFilter]
    [HttpGet("{uid:guid}")]
    public async Task<ActionResult<TopicResponseContract>> GetByUid([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting topic with uid: {TopicUid}.", uid);

        var topic = await topicService.GetByUidAsync(uid, cancellationToken);

        if (topic is null)
        {
            logger.LogWarning("topic with uid {TopicUid} was not found.", uid);
            return NotFound();
        }

        logger.LogInformation("topic received with uid: {TopicUid}.", uid);

        return Ok(topic);
    }
    
    [HttpGet("getPostTopics/{uid:guid}")]
    public async Task<ActionResult<TopicResponseContract>> AddTopic([FromRoute] Guid uid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("getting post topics ");

        var topics = await topicService.GetPostTopicsAsync(uid, cancellationToken);

        logger.LogInformation("got topics for post: {name}. ", uid);

        return Ok(topics);
    }

    [HttpPost]
    public async Task<ActionResult<TopicResponseContract>> Create(
        [FromBody] CreateTopicRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating topic with name {name}.", request.Name);

        var createdTopic = await topicService.CreateAsync(request, cancellationToken);

        logger.LogInformation("Created topic with name: {name}. topic UId: {UId}", createdTopic.Title,
            createdTopic.UId);

        return CreatedAtAction(nameof(GetByUid), new { uid = createdTopic.UId }, createdTopic);
    }
    
    [HttpPost("addTopic/{uid:guid}/ToPost/{postUid:guid}")]
    public async Task<ActionResult<TopicResponseContract>> AddTopic([FromRoute] Guid uid,
        [FromRoute] Guid postUid,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding topic to post {}.", postUid);

        var createdTopic = await topicService.AddTopicToPostAsync(postUid, uid, cancellationToken);

        logger.LogInformation("Added topic to post {postuid}. topic UId: {UId}", postUid, uid);

        return CreatedAtAction(nameof(GetByUid), new { uid = postUid }, createdTopic);
    }

    [TopicExceptionFilter]
    [HttpPut("{uid:guid}")]
    public async Task<ActionResult<TopicResponseContract>> Update(
        [FromRoute] Guid uid,
        [FromBody] UpdateTopicRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating topic with uid: {TopicUid}.", uid);

        var updatedTopic = await topicService.UpdateAsync(uid, request, cancellationToken);

        if (updatedTopic is null)
        {
            logger.LogWarning("topic with uid {TopicUid} was not found for update.", uid);
            return NotFound();
        }

        logger.LogInformation("topic with uid: {TopicUid} updated.", uid);

        return Ok(updatedTopic);
    }

    [TopicExceptionFilter]
    [HttpDelete("{uid:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid uid, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting topic with uid: {TopicUid}.", uid);

        var deleted = await topicService.DeleteAsync(uid, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("topic with uid {TopicUid} was not found for deletion.", uid);
            return NotFound();
        }

        logger.LogInformation("topic with uid: {TopicUid} deleted.", uid);

        return NoContent();
    }
}