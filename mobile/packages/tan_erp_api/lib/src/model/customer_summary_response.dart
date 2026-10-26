//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_summary_response.g.dart';

/// CustomerSummaryResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [displayNameTh] 
/// * [displayNameEn] 
/// * [status] 
@BuiltValue()
abstract class CustomerSummaryResponse implements Built<CustomerSummaryResponse, CustomerSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  @BuiltValueField(wireName: r'status')
  String? get status;

  CustomerSummaryResponse._();

  factory CustomerSummaryResponse([void updates(CustomerSummaryResponseBuilder b)]) = _$CustomerSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerSummaryResponse> get serializer => _$CustomerSummaryResponseSerializer();
}

class _$CustomerSummaryResponseSerializer implements PrimitiveSerializer<CustomerSummaryResponse> {
  @override
  final Iterable<Type> types = const [CustomerSummaryResponse, _$CustomerSummaryResponse];

  @override
  final String wireName = r'CustomerSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameTh != null) {
      yield r'displayNameTh';
      yield serializers.serialize(
        object.displayNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameEn != null) {
      yield r'displayNameEn';
      yield serializers.serialize(
        object.displayNameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerSummaryResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'displayNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameTh = valueDes;
          break;
        case r'displayNameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameEn = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CustomerSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerSummaryResponseBuilder();
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


