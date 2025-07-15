export const get = async (
  input: RequestInfo | URL,
  token?: string
): Promise<Response> => {
  if (token) {
    return fetch(input, {
      method: "GET",
      headers: {
        Authorization: "Bearer " + token,
      },
    });
  } else {
    return fetch(input, {
      method: "GET",
    });
  }
};

export const post = async (
  input: RequestInfo | URL,
  body: any,
  token?: string
): Promise<Response> => {
  const stringified = JSON.stringify(body);

  if (token) {
    return fetch(input, {
      method: "POST",
      body: stringified,
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });
  } else {
    return fetch(input, {
      method: "POST",
      body: stringified,
      headers: {
        "Content-Type": "application/json",
      },
    });
  }
};

export const postForm = async (
  input: RequestInfo | URL,
  body: FormData,
  token?: string
): Promise<Response> => {
  if (token) {
    return fetch(input, {
      method: "POST",
      body,
      headers: {
        Authorization: "Bearer " + token,
      },
    });
  } else {
    return fetch(input, {
      method: "POST",
      body,
    });
  }
};
