using var client = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(5)
};

try
{
    using var response = await client.GetAsync(
        "http://127.0.0.1:8080/health");

    return response.IsSuccessStatusCode ? 0 : 1;
}
catch
{
    return 1;
}
