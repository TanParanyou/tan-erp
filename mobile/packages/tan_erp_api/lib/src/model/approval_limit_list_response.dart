//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/approval_limit_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'approval_limit_list_response.g.dart';

/// ApprovalLimitListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class ApprovalLimitListResponse implements Built<ApprovalLimitListResponse, ApprovalLimitListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<ApprovalLimitResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  AdminPaginationResponse? get pagination;

  ApprovalLimitListResponse._();

  factory ApprovalLimitListResponse([void updates(ApprovalLimitListResponseBuilder b)]) = _$ApprovalLimitListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ApprovalLimitListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ApprovalLimitListResponse> get serializer => _$ApprovalLimitListResponseSerializer();
}

class _$ApprovalLimitListResponseSerializer implements PrimitiveSerializer<ApprovalLimitListResponse> {
  @override
  final Iterable<Type> types = const [ApprovalLimitListResponse, _$ApprovalLimitListResponse];

  @override
  final String wireName = r'ApprovalLimitListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ApprovalLimitListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ApprovalLimitResponse)]),
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
    ApprovalLimitListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ApprovalLimitListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ApprovalLimitResponse)]),
          ) as BuiltList<ApprovalLimitResponse>?;
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
  ApprovalLimitListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ApprovalLimitListResponseBuilder();
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


