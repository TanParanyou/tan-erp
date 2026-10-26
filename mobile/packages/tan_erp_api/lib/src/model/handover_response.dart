//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'handover_response.g.dart';

/// HandoverResponse
///
/// Properties:
/// * [date] 
/// * [signerName] 
/// * [note] 
/// * [warrantyMonths] 
@BuiltValue()
abstract class HandoverResponse implements Built<HandoverResponse, HandoverResponseBuilder> {
  @BuiltValueField(wireName: r'date')
  Date? get date;

  @BuiltValueField(wireName: r'signerName')
  String? get signerName;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'warrantyMonths')
  int? get warrantyMonths;

  HandoverResponse._();

  factory HandoverResponse([void updates(HandoverResponseBuilder b)]) = _$HandoverResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(HandoverResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<HandoverResponse> get serializer => _$HandoverResponseSerializer();
}

class _$HandoverResponseSerializer implements PrimitiveSerializer<HandoverResponse> {
  @override
  final Iterable<Type> types = const [HandoverResponse, _$HandoverResponse];

  @override
  final String wireName = r'HandoverResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    HandoverResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.date != null) {
      yield r'date';
      yield serializers.serialize(
        object.date,
        specifiedType: const FullType(Date),
      );
    }
    if (object.signerName != null) {
      yield r'signerName';
      yield serializers.serialize(
        object.signerName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.warrantyMonths != null) {
      yield r'warrantyMonths';
      yield serializers.serialize(
        object.warrantyMonths,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    HandoverResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required HandoverResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'date':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.date = valueDes;
          break;
        case r'signerName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerName = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'warrantyMonths':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.warrantyMonths = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  HandoverResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = HandoverResponseBuilder();
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


