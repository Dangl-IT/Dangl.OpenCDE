/**
 * OpenCDE Documents API
 * OpenCDE Documents API Specification
 *
 * The version of the OpenAPI document: 1.0
 *
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */
import { Headers } from './headers';
import { MultipartFormData } from './multipartFormData';

/**
 * This model describes how a single part of a multipart file upload should be processed by the client. The upload links may only be valid for a specific time period, after which the links expire. Server vendors should make sure to have a reasonably long expiration date for clients to upload large files
 */
export interface UploadFilePartInstruction {
  url: string;
  http_method: UploadFilePartInstructionHttpMethodEnum;
  additional_headers?: Headers;
  /**
   * Whether or not to include the authorization request header in the file upload request. Including the authorization header with some cloud storage providers might fail the request
   */
  include_authorization?: boolean;
  multipart_form_data?: MultipartFormData;
  /**
   * The inclusive, zero index based start for this part
   */
  content_range_start: number;
  /**
   * The inclusive, zero index based end for this part
   */
  content_range_end: number;
}
export enum UploadFilePartInstructionHttpMethodEnum {
  Post = 'POST',
  Put = 'PUT',
}