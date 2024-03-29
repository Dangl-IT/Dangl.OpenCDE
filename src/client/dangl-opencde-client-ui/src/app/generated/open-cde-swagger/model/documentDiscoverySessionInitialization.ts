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

export interface DocumentDiscoverySessionInitialization {
  /**
   * A CDE UI URL for the client to open in a local browser. The user would search and select documents directly in the CDE
   */
  select_documents_url: string;
  /**
   * `select_documents_url` expiry in seconds
   */
  expires_in: number;
}
