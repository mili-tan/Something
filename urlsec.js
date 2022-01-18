const url = "https://cgi.urlsec.qq.com/index.php?m=check&a=check&url="

async function gatherResponse(response) {
  const { headers } = response
  const contentType = headers.get("content-type") || ""
  if (contentType.includes("application/json")) {
    return JSON.stringify(await response.json())
  }
  else if (contentType.includes("application/text")) {
    return response.text()
  }
  else if (contentType.includes("text/html")) {
    return response.text()
  }
  else {
    return response.text()
  }
}

async function handleRequest(request) {
  const init = {
    headers: {
      "Client-Ip": "192.168.1.100",
      "X-Forwarded-For": "192.168.1.100",
      "Referer": "https://urlsec.qq.com/"
    },
    method: 'POST',
  }
  const response = await fetch(url + new URL(request.url).searchParams.get("name"), init)
  const results = await gatherResponse(response)
  return new Response(results, init)
}

addEventListener("fetch", event => {
  return event.respondWith(handleRequest(event.request))
})