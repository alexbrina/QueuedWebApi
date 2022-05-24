import { sleep } from 'k6';
import http from 'k6/http';

var url = __ENV.SUT_URL || 'http://localhost:5000/Work/Requeue';

export default function () {
  var payload = JSON.stringify({
    data: new Date()
  });

  var params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  sleep(0.100);
  http.post(url, payload, params);
}
