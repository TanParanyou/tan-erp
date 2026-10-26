//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'add_on_response.g.dart';

/// AddOnResponse
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [amount] 
/// * [perLine] 
@BuiltValue()
abstract class AddOnResponse implements Built<AddOnResponse, AddOnResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'perLine')
  bool? get perLine;

  AddOnResponse._();

  factory AddOnResponse([void updates(AddOnResponseBuilder b)]) = _$AddOnResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AddOnResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AddOnResponse> get serializer => _$AddOnResponseSerializer();
}

class _$AddOnResponseSerializer implements PrimitiveSerializer<AddOnResponse> {
  @override
  final Iterable<Type> types = const [AddOnResponse, _$AddOnResponse];

  @override
  final String wireName = r'AddOnResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AddOnResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.perLine != null) {
      yield r'perLine';
      yield serializers.serialize(
        object.perLine,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AddOnResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AddOnResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'perLine':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.perLine = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AddOnResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AddOnResponseBuilder();
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


