//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/opportunity_work_image_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_work_image_list_response.g.dart';

/// OpportunityWorkImageListResponse
///
/// Properties:
/// * [items] 
/// * [nextCursor] 
@BuiltValue()
abstract class OpportunityWorkImageListResponse implements Built<OpportunityWorkImageListResponse, OpportunityWorkImageListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<OpportunityWorkImageResponse>? get items;

  @BuiltValueField(wireName: r'nextCursor')
  String? get nextCursor;

  OpportunityWorkImageListResponse._();

  factory OpportunityWorkImageListResponse([void updates(OpportunityWorkImageListResponseBuilder b)]) = _$OpportunityWorkImageListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityWorkImageListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityWorkImageListResponse> get serializer => _$OpportunityWorkImageListResponseSerializer();
}

class _$OpportunityWorkImageListResponseSerializer implements PrimitiveSerializer<OpportunityWorkImageListResponse> {
  @override
  final Iterable<Type> types = const [OpportunityWorkImageListResponse, _$OpportunityWorkImageListResponse];

  @override
  final String wireName = r'OpportunityWorkImageListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityWorkImageListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityWorkImageResponse)]),
      );
    }
    if (object.nextCursor != null) {
      yield r'nextCursor';
      yield serializers.serialize(
        object.nextCursor,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityWorkImageListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityWorkImageListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(OpportunityWorkImageResponse)]),
          ) as BuiltList<OpportunityWorkImageResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'nextCursor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nextCursor = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OpportunityWorkImageListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityWorkImageListResponseBuilder();
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


