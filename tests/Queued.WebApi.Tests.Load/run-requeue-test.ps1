$concurrent_clients = 12
$execution_time = "4s"

# run tests
Write-Output "starting load test..."
k6 run --vus $concurrent_clients --duration $execution_time .\k6-requeue-endpoint.js
