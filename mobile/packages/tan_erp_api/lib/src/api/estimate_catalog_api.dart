//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

import 'package:built_value/json_object.dart';
import 'package:built_value/serializer.dart';
import 'package:dio/dio.dart';

import 'package:tan_erp_api/src/api_util.dart';
import 'package:tan_erp_api/src/model/api_problem_details.dart';
import 'package:tan_erp_api/src/model/estimate_catalog_response.dart';
import 'package:tan_erp_api/src/model/estimate_unit_options_response.dart';

class EstimateCatalogApi {

  final Dio _dio;

  final Serializers _serializers;

  const EstimateCatalogApi(this._dio, this._serializers);

  /// apiV1EstimateCatalogItemsGet
  /// 
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [search] 
  /// * [itemType] 
  /// * [categoryId] 
  /// * [brandId] 
  /// * [attributeKey] 
  /// * [attributeValue] 
  /// * [hasCost] 
  /// * [cursor] 
  /// * [pageSize] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [EstimateCatalogResponse] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<EstimateCatalogResponse>> apiV1EstimateCatalogItemsGet({ 
    required String branchId,
    String? search,
    String? itemType,
    String? categoryId,
    String? brandId,
    String? attributeKey,
    String? attributeValue,
    bool? hasCost,
    String? cursor,
    int? pageSize = 25,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/estimate-catalog/items';
    final _options = Options(
      method: r'GET',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
            'type': 'http',
            'scheme': 'Bearer',
            'name': 'Bearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _queryParameters = <String, dynamic>{
      r'branchId': encodeQueryParameter(_serializers, branchId, const FullType(String)),
      if (search != null) r'search': encodeQueryParameter(_serializers, search, const FullType(String)),
      if (itemType != null) r'itemType': encodeQueryParameter(_serializers, itemType, const FullType(String)),
      if (categoryId != null) r'categoryId': encodeQueryParameter(_serializers, categoryId, const FullType(String)),
      if (brandId != null) r'brandId': encodeQueryParameter(_serializers, brandId, const FullType(String)),
      if (attributeKey != null) r'attributeKey': encodeQueryParameter(_serializers, attributeKey, const FullType(String)),
      if (attributeValue != null) r'attributeValue': encodeQueryParameter(_serializers, attributeValue, const FullType(String)),
      if (hasCost != null) r'hasCost': encodeQueryParameter(_serializers, hasCost, const FullType(bool)),
      if (cursor != null) r'cursor': encodeQueryParameter(_serializers, cursor, const FullType(String)),
      if (pageSize != null) r'pageSize': encodeQueryParameter(_serializers, pageSize, const FullType(int)),
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    EstimateCatalogResponse? _responseData;

    try {
      final rawResponse = _response.data;
      _responseData = rawResponse == null ? null : _serializers.deserialize(
        rawResponse,
        specifiedType: const FullType(EstimateCatalogResponse),
      ) as EstimateCatalogResponse;

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<EstimateCatalogResponse>(
      data: _responseData,
      headers: _response.headers,
      isRedirect: _response.isRedirect,
      requestOptions: _response.requestOptions,
      redirects: _response.redirects,
      statusCode: _response.statusCode,
      statusMessage: _response.statusMessage,
      extra: _response.extra,
    );
  }

  /// apiV1EstimateCatalogItemsItemIdUnitsGet
  /// 
  ///
  /// Parameters:
  /// * [itemId] 
  /// * [branchId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [EstimateUnitOptionsResponse] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<EstimateUnitOptionsResponse>> apiV1EstimateCatalogItemsItemIdUnitsGet({ 
    required String itemId,
    required String branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/estimate-catalog/items/{itemId}/units'.replaceAll('{' r'itemId' '}', encodeQueryParameter(_serializers, itemId, const FullType(String)).toString());
    final _options = Options(
      method: r'GET',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
            'type': 'http',
            'scheme': 'Bearer',
            'name': 'Bearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _queryParameters = <String, dynamic>{
      r'branchId': encodeQueryParameter(_serializers, branchId, const FullType(String)),
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    EstimateUnitOptionsResponse? _responseData;

    try {
      final rawResponse = _response.data;
      _responseData = rawResponse == null ? null : _serializers.deserialize(
        rawResponse,
        specifiedType: const FullType(EstimateUnitOptionsResponse),
      ) as EstimateUnitOptionsResponse;

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<EstimateUnitOptionsResponse>(
      data: _responseData,
      headers: _response.headers,
      isRedirect: _response.isRedirect,
      requestOptions: _response.requestOptions,
      redirects: _response.redirects,
      statusCode: _response.statusCode,
      statusMessage: _response.statusMessage,
      extra: _response.extra,
    );
  }

}
