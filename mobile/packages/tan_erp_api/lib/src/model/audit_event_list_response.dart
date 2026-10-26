//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/audit_event_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'audit_event_list_response.g.dart';

/// AuditEventListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class AuditEventListResponse implements Built<AuditEventListResponse, AuditEventListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AuditEventResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  AdminPaginationResponse? get pagination;

  AuditEventListResponse._();

  factory AuditEventListResponse([void updates(AuditEventListResponseBuilder b)]) = _$AuditEventListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AuditEventListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AuditEventListResponse> get serializer => _$AuditEventListResponseSerializer();
}

class _$AuditEventListResponseSerializer implements PrimitiveSerializer<AuditEventListResponse> {
  @override
  final Iterable<Type> types = const [AuditEventListResponse, _$AuditEventListResponse];

  @override
  final String wireName = r'AuditEventListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AuditEventListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AuditEventResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(AdminPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AuditEventListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AuditEventListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AuditEventResponse)]),
          ) as BuiltList<AuditEventResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminPaginationResponse),
          ) as AdminPaginationResponse?;
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
  AuditEventListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AuditEventListResponseBuilder();
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


