//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'account_mapping_request.g.dart';

/// AccountMappingRequest
///
/// Properties:
/// * [kind] 
/// * [debitAccountCode] 
/// * [debitAccountName] 
/// * [creditAccountCode] 
/// * [creditAccountName] 
@BuiltValue()
abstract class AccountMappingRequest implements Built<AccountMappingRequest, AccountMappingRequestBuilder> {
  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'debitAccountCode')
  String? get debitAccountCode;

  @BuiltValueField(wireName: r'debitAccountName')
  String? get debitAccountName;

  @BuiltValueField(wireName: r'creditAccountCode')
  String? get creditAccountCode;

  @BuiltValueField(wireName: r'creditAccountName')
  String? get creditAccountName;

  AccountMappingRequest._();

  factory AccountMappingRequest([void updates(AccountMappingRequestBuilder b)]) = _$AccountMappingRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AccountMappingRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AccountMappingRequest> get serializer => _$AccountMappingRequestSerializer();
}

class _$AccountMappingRequestSerializer implements PrimitiveSerializer<AccountMappingRequest> {
  @override
  final Iterable<Type> types = const [AccountMappingRequest, _$AccountMappingRequest];

  @override
  final String wireName = r'AccountMappingRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AccountMappingRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.debitAccountCode != null) {
      yield r'debitAccountCode';
      yield serializers.serialize(
        object.debitAccountCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.debitAccountName != null) {
      yield r'debitAccountName';
      yield serializers.serialize(
        object.debitAccountName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditAccountCode != null) {
      yield r'creditAccountCode';
      yield serializers.serialize(
        object.creditAccountCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditAccountName != null) {
      yield r'creditAccountName';
      yield serializers.serialize(
        object.creditAccountName,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AccountMappingRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AccountMappingRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'debitAccountCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.debitAccountCode = valueDes;
          break;
        case r'debitAccountName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.debitAccountName = valueDes;
          break;
        case r'creditAccountCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creditAccountCode = valueDes;
          break;
        case r'creditAccountName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creditAccountName = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AccountMappingRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AccountMappingRequestBuilder();
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


