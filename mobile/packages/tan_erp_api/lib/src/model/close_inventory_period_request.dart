//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'close_inventory_period_request.g.dart';

/// CloseInventoryPeriodRequest
///
/// Properties:
/// * [periodYear] 
/// * [periodMonth] 
/// * [note] 
@BuiltValue()
abstract class CloseInventoryPeriodRequest implements Built<CloseInventoryPeriodRequest, CloseInventoryPeriodRequestBuilder> {
  @BuiltValueField(wireName: r'periodYear')
  int? get periodYear;

  @BuiltValueField(wireName: r'periodMonth')
  int? get periodMonth;

  @BuiltValueField(wireName: r'note')
  String? get note;

  CloseInventoryPeriodRequest._();

  factory CloseInventoryPeriodRequest([void updates(CloseInventoryPeriodRequestBuilder b)]) = _$CloseInventoryPeriodRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CloseInventoryPeriodRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CloseInventoryPeriodRequest> get serializer => _$CloseInventoryPeriodRequestSerializer();
}

class _$CloseInventoryPeriodRequestSerializer implements PrimitiveSerializer<CloseInventoryPeriodRequest> {
  @override
  final Iterable<Type> types = const [CloseInventoryPeriodRequest, _$CloseInventoryPeriodRequest];

  @override
  final String wireName = r'CloseInventoryPeriodRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CloseInventoryPeriodRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.periodYear != null) {
      yield r'periodYear';
      yield serializers.serialize(
        object.periodYear,
        specifiedType: const FullType(int),
      );
    }
    if (object.periodMonth != null) {
      yield r'periodMonth';
      yield serializers.serialize(
        object.periodMonth,
        specifiedType: const FullType(int),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CloseInventoryPeriodRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CloseInventoryPeriodRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'periodYear':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.periodYear = valueDes;
          break;
        case r'periodMonth':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.periodMonth = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CloseInventoryPeriodRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CloseInventoryPeriodRequestBuilder();
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


