//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/installation_list_item_response.dart';
import 'package:tan_erp_api/src/model/service_pagination_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'installations_list_response.g.dart';

/// InstallationsListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class InstallationsListResponse implements Built<InstallationsListResponse, InstallationsListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<InstallationListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ServicePaginationResponse? get pagination;

  InstallationsListResponse._();

  factory InstallationsListResponse([void updates(InstallationsListResponseBuilder b)]) = _$InstallationsListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InstallationsListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InstallationsListResponse> get serializer => _$InstallationsListResponseSerializer();
}

class _$InstallationsListResponseSerializer implements PrimitiveSerializer<InstallationsListResponse> {
  @override
  final Iterable<Type> types = const [InstallationsListResponse, _$InstallationsListResponse];

  @override
  final String wireName = r'InstallationsListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InstallationsListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(InstallationListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(ServicePaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InstallationsListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InstallationsListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(InstallationListItemResponse)]),
          ) as BuiltList<InstallationListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePaginationResponse),
          ) as ServicePaginationResponse?;
          if (valueDes == null) continue;
          result.pagination.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InstallationsListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InstallationsListResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


