//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_line_response.g.dart';

/// BillingLineResponse
///
/// Properties:
/// * [id] 
/// * [lineNumber] 
/// * [description] 
/// * [taxBaseAmount] 
/// * [vatRate] 
/// * [vatAmount] 
/// * [totalAmount] 
@BuiltValue()
abstract class BillingLineResponse implements Built<BillingLineResponse, BillingLineResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'lineNumber')
  int? get lineNumber;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'taxBaseAmount')
  double? get taxBaseAmount;

  @BuiltValueField(wireName: r'vatRate')
  double? get vatRate;

  @BuiltValueField(wireName: r'vatAmount')
  double? get vatAmount;

  @BuiltValueField(wireName: r'totalAmount')
  double? get totalAmount;

  BillingLineResponse._();

  factory BillingLineResponse([void updates(BillingLineResponseBuilder b)]) = _$BillingLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingLineResponse> get serializer => _$BillingLineResponseSerializer();
}

class _$BillingLineResponseSerializer implements PrimitiveSerializer<BillingLineResponse> {
  @override
  final Iterable<Type> types = const [BillingLineResponse, _$BillingLineResponse];

  @override
  final String wireName = r'BillingLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.lineNumber != null) {
      yield r'lineNumber';
      yield serializers.serialize(
        object.lineNumber,
        specifiedType: const FullType(int),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxBaseAmount != null) {
      yield r'taxBaseAmount';
      yield serializers.serialize(
        object.taxBaseAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.vatRate != null) {
      yield r'vatRate';
      yield serializers.serialize(
        object.vatRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.vatAmount != null) {
      yield r'vatAmount';
      yield serializers.serialize(
        object.vatAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalAmount != null) {
      yield r'totalAmount';
      yield serializers.serialize(
        object.totalAmount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingLineResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'lineNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lineNumber = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'taxBaseAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxBaseAmount = valueDes;
          break;
        case r'vatRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatRate = valueDes;
          break;
        case r'vatAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatAmount = valueDes;
          break;
        case r'totalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingLineResponseBuilder();
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


