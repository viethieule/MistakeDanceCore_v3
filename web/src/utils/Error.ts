/**
 * Returns the API error message
 *
 * @param error Error object
 * @returns Error message or blank if no error
 */
export function getErrorMessage(error: any): string {
  if (!error) {
    return "";
  }

  // API error?
  if (error.response) {
    if (error.response.data) {
      if (error.response.data.errorMessage) {
        return error.response.data.errorMessage;
      } else if (error.response.data.errorCode) {
        return "ErrorCode: " + error.response.data.errorCode;
      }
    }
  }

  // Exception?
  if (error.message) {
    return error.message;
  }

  return error;
}
