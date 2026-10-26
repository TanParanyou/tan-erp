//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/mrp_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/mrp_run_list_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_run_list_response.g.dart';

/// MrpRunListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class MrpRunListResponse implements Built<MrpRunListResponse, MrpRunListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<MrpRunListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  MrpPaginationResponse? get pagination;

  MrpRunListResponse._();

  factory MrpRunListResponse([void updates(MrpRunListResponseBuilder b)]) = _$MrpRunListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpRunListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpRunListResponse> get serializer => _$MrpRunListResponseSerializer();
}

class _$MrpRunListResponseSerializer implements PrimitiveSerializer<MrpRunListResponse> {
  @override
  final Iterable<Type> types = const [MrpRunListResponse, _$MrpRunListResponse];

  @override
  final String wireName = r'MrpRunListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpRunListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MrpRunListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(MrpPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpRunListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpRunListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MrpRunListItemResponse)]),
          ) as BuiltList<MrpRunListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpPaginationResponse),
          ) as MrpPaginationResponse?;
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
  MrpRunListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpRunListResponseBuilder();
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


