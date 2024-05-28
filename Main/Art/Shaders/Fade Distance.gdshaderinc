float fade_distance(float opacity, float min_distance, float max_distance, vec3 camera_position, vec3 node_position)
{
    float distance = distance(camera_position, node_position);
    float remapped_distance = 1.0 - (distance - min_distance) / (max_distance - min_distance);
    float clamped_distance = clamp(-remapped_distance, 0.0, 1.0);
    return opacity * clamped_distance;
}