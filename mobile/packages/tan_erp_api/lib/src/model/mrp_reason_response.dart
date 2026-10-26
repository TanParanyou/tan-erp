//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_reason_response.g.dart';

/// MrpReasonResponse
///
/// Properties:
/// * [sourceType] 
/// * [sourceRef] 
/// * [quantity] 
/// * [needBy] 
@BuiltValue()
abstract class MrpReasonResponse implements Built<MrpReasonResponse, MrpReasonResponseBuilder> {
  @BuiltValueField(wireName: r'sourceType')
  String? get sourceType;

  @BuiltValueField(wireName: r'sourceRef')
  String? get sourceRef;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'needBy')
  Date? get needBy;

  MrpReasonResponse._();

  factory MrpReasonResponse([void updates(MrpReasonResponseBuilder b)]) = _$MrpReasonResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpReasonResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpReasonResponse> get serializer => _$MrpReasonResponseSerializer();
}

class _$MrpReasonResponseSerializer implements PrimitiveSerializer<MrpReasonResponse> {
  @override
  final Iterable<Type> types = const [MrpReasonResponse, _$MrpReasonResponse];

  @override
  final String wireName = r'MrpReasonResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpReasonResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sourceType != null) {
      yield r'sourceType';
      yield serializers.serialize(
        object.sourceType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceRef != null) {
      yield r'sourceRef';
      yield serializers.serialize(
        object.sourceRef,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.needBy != null) {
      yield r'needBy';
      yield serializers.serialize(
        object.needBy,
        specifiedType: const FullType(Date),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpReasonResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpReasonResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sourceType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceType = valueDes;
          break;
        case r'sourceRef':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceRef = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'needBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.needBy = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpReasonResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpReasonResponseBuilder();
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


