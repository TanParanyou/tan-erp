//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/assigned_survey_sync_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'assigned_survey_sync_list_response.g.dart';

/// AssignedSurveySyncListResponse
///
/// Properties:
/// * [items] 
/// * [serverTimeUtc] 
@BuiltValue()
abstract class AssignedSurveySyncListResponse implements Built<AssignedSurveySyncListResponse, AssignedSurveySyncListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<AssignedSurveySyncItemResponse>? get items;

  @BuiltValueField(wireName: r'serverTimeUtc')
  DateTime? get serverTimeUtc;

  AssignedSurveySyncListResponse._();

  factory AssignedSurveySyncListResponse([void updates(AssignedSurveySyncListResponseBuilder b)]) = _$AssignedSurveySyncListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AssignedSurveySyncListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AssignedSurveySyncListResponse> get serializer => _$AssignedSurveySyncListResponseSerializer();
}

class _$AssignedSurveySyncListResponseSerializer implements PrimitiveSerializer<AssignedSurveySyncListResponse> {
  @override
  final Iterable<Type> types = const [AssignedSurveySyncListResponse, _$AssignedSurveySyncListResponse];

  @override
  final String wireName = r'AssignedSurveySyncListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AssignedSurveySyncListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AssignedSurveySyncItemResponse)]),
      );
    }
    if (object.serverTimeUtc != null) {
      yield r'serverTimeUtc';
      yield serializers.serialize(
        object.serverTimeUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AssignedSurveySyncListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AssignedSurveySyncListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AssignedSurveySyncItemResponse)]),
          ) as BuiltList<AssignedSurveySyncItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'serverTimeUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.serverTimeUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AssignedSurveySyncListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AssignedSurveySyncListResponseBuilder();
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


